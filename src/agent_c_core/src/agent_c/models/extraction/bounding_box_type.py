from enum import Enum

class BoundingBoxType(Enum):
    bbox = 'bbox'
    legend = 'legend'
    table_title = 'table_title'
    table_header = 'table_header'
    table = 'table'
    table_row = 'table_row'
    page_footer = 'page_footer'
    page_header = 'page_header'

