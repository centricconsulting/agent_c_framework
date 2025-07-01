import base64
import io
import os
import json
import mammoth
import logging
import traceback

from PIL import Image
from email import policy
from pathlib import Path
from email.parser import BytesParser
from typing import Union, Any, Optional
from email.message import Message, EmailMessage


from agent_c.util.loaders.pdf_form_loader import PDFFormLoader
from agent_c.util.loaders.simple_document_loader import SimpleDocumentLoader
from agent_c.util.tesseract import extract_text_from_image

class EmlLoader:
    """A class for loading and processing .eml files."""

    def __init__(self, loader: Union[PDFFormLoader, None] = None):
        self.form_loader = loader
        self.doc_loader = SimpleDocumentLoader()
        self.min_image_short_size = 296
        self.min_image_long_size = 600

        if self.form_loader is None:
            stop_regexes = ["^\\*\\*APPLICABLE\\s+IN ", "^APPLICABLE\\s+IN "]
            self.form_loader = PDFFormLoader(stop_regexes=stop_regexes)

    async def augment_email_data(self, email_data: dict[str, Any]) -> dict[str, Any]:
        return email_data

    async def extract_email_data(self, eml_path: str, output_folder: Optional[str]) -> Optional[dict[str, Any]]:
        base_name = os.path.basename(eml_path.replace(" ", "_"))
        msg = self.__load_eml_file(eml_path)
        if msg is None:
            return None

        return await self.__extract_msg_data(msg, base_name, output_folder)

    async def __extract_msg_data(self, msg: EmailMessage, base_name: str, output_folder: Optional[str], top_level: Optional[bool] = True) -> Optional[dict[str, Any]]:
        """Extracts data from an .eml file and saves attachments and email data in a specified output folder.

        Args:
            msg (EmailMessage): The email message object.
            base_name (str): The base name of the .eml file.
            output_folder (Optional[str]): The path to the output folder to save extracted data. If None, no data is saved.

        Returns:
            Optional[dict[str, Any]]: A dictionary containing email metadata and attachments, or None if an error occurs.
        """
        msg_data = self.__extract_msg_meta(msg)
        if output_folder is not None:
            subfolder_name = os.path.splitext(base_name)[0]
            subfolder_path = os.path.join(output_folder, subfolder_name)
            os.makedirs(subfolder_path, exist_ok=True)
        else:
            subfolder_path = None

        msg_data['attachments'] = await self.__process_attachments(msg, subfolder_path)

        msg_data = self.flatten_email(msg_data)


        attachments = msg_data.pop('attachments')

        if output_folder is not None:

            if attachments:
                msg_data['attachments'] = {
                    file_type: [
                        {key: value for key, value in attachment.items() if key != 'content'}
                        for attachment in attachments[file_type]
                    ]
                    for file_type in attachments
                }

            try:
                with open(os.path.join(subfolder_path, 'email_data.json'), 'w', encoding="utf-8") as json_file:
                    json.dump(msg_data, json_file, indent=4)
            except Exception as e:
                logging.error(f"An error occurred while saving email data: {e}")

            if attachments:
                msg_data['attachments'] = attachments
                try:
                    with open(os.path.join(subfolder_path, 'email_data_full.json'), 'w', encoding="utf-8") as json_file:
                        json.dump(msg_data, json_file, indent=4)
                except Exception as e:
                    logging.error(f"An error occurred while saving email data: {e}")

        if top_level:
            msg_data = await self.augment_email_data(msg_data)

        return msg_data

    def extract_image(self, image_bytes):
        try:
            image = Image.open(io.BytesIO(image_bytes))
            short_side = min(image.size)
            long_side = max(image.size)
            if short_side < self.min_image_short_size or long_side < self.min_image_long_size:
                return None
        except Exception as e:
            logging.error(f"An error occurred while opening image: {e}", e)
            return None

        return image

    async def __process_attachments(self, msg: EmailMessage, subfolder_path: Optional[str]) -> dict[str, list[dict[str, Any]]]:
        """Processes email attachments and saves them to a specified subfolder.

        Args:
            msg (Message): The email message object.
            subfolder_path (Optional[str]): The path to the subfolder to save attachments. If None, attachments are not saved.

        Returns:
            dict[str, list[dict[str, Any]]]: A dictionary categorizing attachments by their main type.
        """
        attachments = {}
        def convert_image(image):
            return {
                "src": ""
            }

        for part in msg.iter_attachments():
            attachment_filename = part.get_filename()
            if attachment_filename:
                attachment_filename = attachment_filename.replace(" ", "_")

            part_text = ''
            part_type = part.get_content_maintype()
            type_files = attachments.get(part_type.lower(), [])

            if part_type == 'image':
                part_content = part.get_payload().replace("\n", "")
                image = self.extract_image(part.get_payload(decode=True))

                if image is None:
                    logging.info(f"Image {attachment_filename} skipped due to size.")
                    continue
                part_text = extract_text_from_image(image)

            elif part_type == 'message':
                fwd_no = len(type_files) + 1
                base_name = f"extracted_forward_{fwd_no}"
                attachment_filename = f"{base_name}.eml"
                part_content = await self.__extract_msg_data(part.get_content(), base_name, subfolder_path, False)
            else:
                if attachment_filename is not None:
                    _, part_type = os.path.splitext(attachment_filename)
                    part_type = part_type[1:].lower()
                else:
                    part_type = part.get_content_type()



            if subfolder_path is not None:
                self.__write_attachment(part, attachment_filename, subfolder_path)

            if part_type == 'pdf':
                try:
                    pdf_bytes = io.BytesIO(part.get_payload(decode=True))
                    part_text = self.form_loader.extract_content_pages(pdf_bytes)
                    part_content = part.get_payload().replace("\n", "")

                    if "".join(part_text).strip() == "":
                        page_images = self.doc_loader.extract_images_from_pdf(pdf_bytes, attachment_filename)
                        attached_images = attachments.get('image', [])
                        for page in page_images:
                            for img_64 in page:
                                img_bytes = io.BytesIO(base64.b64decode(img_64['contents']))
                                image = self.extract_image(img_bytes.read())
                                if image is not None:
                                    if subfolder_path is not None:
                                        self.__write_attachment_bytes(base64.b64decode(img_64['contents']), img_64['filename'], subfolder_path)

                                    img_text = extract_text_from_image(image)
                                    if img_text is not None and len(img_text) > 1:
                                        attached_images.append({"filename": img_64['filename'], "content": img_64['contents'], "text": img_text})
                                    else:
                                        attached_images.append({"filename": img_64['filename'], "content": img_64['contents']})
                        part_text = None
                        attachments['image'] = attached_images


                except Exception as e:
                    logging.error(f"An error occurred while extracting PDF content: {e}")
                    logging.error(traceback.format_exc())


            elif part_type == 'docx':
                style_map = """
                p[style-name='header'] => h2:fresh
                p[style-name='Header'] => h1:fresh
                """
                doc_bytes = io.BytesIO(part.get_payload(decode=True))
                result = mammoth.convert_to_markdown(doc_bytes,
                                                     convert_image=mammoth.images.img_element(convert_image),
                                                     style_map=style_map)
                part_text = result.value

            type_files = attachments.get(part_type.lower(), [])

            if part_text is not None and len(part_text) > 0:
                type_files.append({"filename": attachment_filename, "content": part_content, "text": part_text})
            else:
                type_files.append({"filename": attachment_filename, "content": part_content})

            attachments[part_type.lower()] = type_files

        return attachments

    @staticmethod
    def __load_eml_file(eml_path: str) -> Optional[EmailMessage]:
        """Loads an .eml file and returns the email message object.

        Args:
            eml_path (str): The path to the .eml file.

        Returns:
            Optional[EmailMessage]: The email message object, or None if an error occurs.
        """
        try:
            logging.info(f"Loading {eml_path}")
            with open(eml_path, 'rb') as file:
                msg = BytesParser(policy=policy.default).parse(file)
            return msg
        except FileNotFoundError:
            logging.error(f"The file {eml_path} does not exist.")
        except IsADirectoryError:
            logging.error(f"{eml_path} is a directory, expected a file.")
        except Exception as e:
            logging.error(f"A loading error occurred: {e}")


        return None

    @staticmethod
    def __write_attachment_bytes(attachment_bytes, attachment_filename: str,  subfolder_path: str) -> None:
        try:
            if attachment_filename:
                attachment_filename = attachment_filename.replace(" ", "_")
                attachment_path = os.path.join(subfolder_path, attachment_filename)

                with open(attachment_path, 'wb') as attachment_file:
                        attachment_file.write(attachment_bytes)

                logging.info(f"Attachment {attachment_filename} saved.")
            else:
                logging.info("Attachment with no filename.")
        except Exception as e:
            logging.error(f"An error occurred while saving {attachment_filename}: {e}")

    def __write_attachment(self, part, attachment_filename: str,  subfolder_path: str) -> None:
        """Writes an email attachment to a specified subfolder.

        Args:
            part : The email message part containing the attachment.
            subfolder_path (str): The path to the subfolder where the attachment will be saved.

        Returns:
            None
        """
        if part.get_content_type() == 'message/rfc822':
            self.__write_attachment_bytes(bytes(part.get_payload(0)), attachment_filename, subfolder_path)
        else:
            self.__write_attachment_bytes(part.get_payload(decode=True), attachment_filename, subfolder_path)


    def __extract_msg_meta(self, msg: EmailMessage) -> dict[str, Any]:
        """Extracts metadata from an email message.

        Args:
            msg (Message): The email message object.

        Returns:
            dict[str, Any]: A dictionary containing the extracted metadata.
        """
        msg_data = {
            'subject': msg['subject'],
            'sender': msg['from'],
            'date': msg['date'],
            'body_text': ''
        }

        # Extract body text - only non-multipart or plain text parts
        if msg.is_multipart():
            for part in msg.walk():
                content_type = part.get_content_type()
                if content_type == 'text/plain' and not part.is_attachment():
                    msg_data['body_text'] += part.get_content().strip()
        else:
            msg_data['body_text'] = msg.get_content().strip()

        msg_data['body_text'] = self.__clean_body_text(msg_data['body_text'])


        return msg_data

    def __clean_body_text(self, body_text: str) -> str:
        lines = body_text.replace("\u200c ", "").replace("\r\n", "\n").split("\n")
        good_lines = []
        websites = ['facebook', 'twitter', 'x.com', 'instagram', 'linkedin', 'youtube', 'pinterest', 'tiktok', 'snapchat', 'redatatech']
        boilerplate = ['intended recipient', '________________________________', 'information security warning', 'this is an external email',
                       '[cid:', 'sent from my iphone']

        for line in lines:
            ll = line.lower()
            if self.contains_any_phrase(ll, websites + boilerplate):
                continue
            good_lines.append(line)

        output = "\n".join(good_lines)
        drop_symbols = ["\u200b", "\u202f"]


        for symbol in drop_symbols:
            output = output.replace(symbol, "")
        return output

    @staticmethod
    def contains_any_phrase(input_string, phrases):
        return any(phrase in input_string for phrase in phrases)

    async def process_eml_files(self, input_folder: str, output_folder: str) -> None:
        """Processes all .eml files in the input folder and extracts their data to the output folder.

        Args:
            input_folder (str): The path to the input folder containing .eml files.
            output_folder (str): The path to the output folder where extracted data will be saved.

        Returns:
            None
        """
        # Ensure the input folder exists
        if not os.path.exists(input_folder):
            logging.error(f"Input folder {input_folder} does not exist.")
            return

        # Ensure the output folder exists
        if not os.path.exists(output_folder):
            os.makedirs(output_folder)

        # Iterate over all .eml files in the input folder
        input_path = Path(input_folder)
        for eml_file in input_path.glob('*.eml'):
            try:
                await self.extract_email_data(str(eml_file), output_folder)
            except Exception as e:
                logging.error(f"An error occurred while processing {eml_file}: {e}")
                logging.error(traceback.format_exc())
            else:
                logging.info(f"Processed {eml_file} successfully.")


    def __forwarded_body_text(self, msg: dict) -> str:
        try:
            body_text = ("--Begin Forwarded Message--\n\n"
                        f"From: {msg['sender']}\nDate: {msg['date']}\nSubject: {msg['subject']}\n\n"
                        f"{msg['body_text']}\n\n--End Forwarded Message--\n")
        except Exception as e:
            logging.error(f"An error occurred while creating forwarded message: {e}")
            body_text = ''

        return body_text

    def flatten_email(self, msg_data: dict) -> dict:
        """
        Flatten the email structure by appending the body text and attachments from nested messages.
        """
        body_text = msg_data.get('body_text', '')
        attachments = msg_data.get('attachments', {})

        # Check for nested messages
        nested_msgs = msg_data.get('attachments', {}).get('message', [])
        for nested_msg in nested_msgs:
            temp_flattened = self.flatten_email(nested_msg['content'])
            #nested_body_text = temp_flattened['body_text']
            body_text += self.__forwarded_body_text(temp_flattened)

            # Merge attachments from nested messages
            nested_attachments = temp_flattened['attachments']
            attachments = {**attachments, **nested_attachments}

        msg_data['body_text'] = body_text
        msg_data['attachments'] = attachments

        return msg_data



