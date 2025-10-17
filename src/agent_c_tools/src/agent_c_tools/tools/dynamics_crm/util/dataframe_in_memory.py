import io
import openpyxl # noqa
import pandas as pd

def create_excel_in_memory(dataframe):
    # Create a BytesIO object
    excel_buffer = io.BytesIO()

    # Use ExcelWriter to write to the BytesIO object
    with pd.ExcelWriter(excel_buffer, engine='openpyxl') as writer:
        dataframe.to_excel(writer, index=False, sheet_name='Sheet1')

    # Reset the buffer's position to the start
    excel_buffer.seek(0)

    # Return the BytesIO object
    return excel_buffer


# Example usage:
# df = pd.DataFrame({'A': [1, 2, 3], 'B': [4, 5, 6]})
# excel_file = create_excel_in_memory(df)

# Now you have an Excel file in memory, stored in excel_file
# You can return this directly in a web application,
# or save it to disk if needed:
# with open('output.xlsx', 'wb') as f:
#     f.write(excel_file.getvalue())
