from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--id", prompt="id", required=True)
def generate_customer_bookmarks(aws: str, csv: str, silent: bool,
                                id: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    id = report_common.remove_quotes(id)

    csv_filename = csv
    field_names = [
        "customerId", "sk", "createDate", "description", "folder", "title", "updateDate", "url"
    ]
    key_condition = Key(f'customerId').eq(f'{id}')
    filter_expression = \
        Attr('title').begins_with('AWS') & \
        Attr('url').begins_with('https:')
    report_common.generate_report(
        aws_profile=aws,
        table_name=f'CustomerBookmark', index_name=None,
        sort_column=f'createDate', reverse_sort=True,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=filter_expression,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_customer_bookmarks()
