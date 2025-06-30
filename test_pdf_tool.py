import asyncio
import base64
import json
from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import letter
from io import BytesIO
from agent_c_demo.pdf_converter.tool import PDFConverterTools

async def test_real_pdf():
    # Create a simple test PDF
    buffer = BytesIO()
    p = canvas.Canvas(buffer, pagesize=letter)
    p.drawString(100, 750, 'Hello, this is a test PDF!')
    p.drawString(100, 730, 'This is page 1 content.')
    p.showPage()
    p.drawString(100, 750, 'This is page 2!')
    p.drawString(100, 730, 'More content on page 2.')
    p.save()
    
    pdf_bytes = buffer.getvalue()
    pdf_base64 = base64.b64encode(pdf_bytes).decode('utf-8')
    
    tool = PDFConverterTools()
    result = await tool.pdf_to_json(
        pdf_content=pdf_base64,
        include_metadata=True,
        extract_by_page=True
    )
    
    result_data = json.loads(result)
    print('✅ PDF Conversion Result:')
    print(f'Success: {result_data["success"]}')
    print(f'Total Pages: {result_data["total_pages"]}')
    
    for page in result_data['content']['pages']:
        print(f'Page {page["page_number"]}: {page["text"][:100]}...')

asyncio.run(test_real_pdf())
