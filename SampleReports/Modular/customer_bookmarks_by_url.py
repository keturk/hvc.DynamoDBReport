from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--url", prompt="url", required=True)
def generate_customer_bookmarks_by_url(aws: str, csv: str, silent: bool,
                                       url: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    url = report_common.remove_quotes(url)

    csv_filename = csv
    field_names = [
        "customerId", "sk", "createDate", "description", "folder", "title", "updateDate", "url"
    ]
    key_condition = Key(f'url').eq(f'{url}')

    report_common.generate_report(
        aws_profile=aws,
        table_name=f'CustomerBookmark', index_name=f'ByUrl',
        sort_column=f'createDate', reverse_sort=True,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=None,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_customer_bookmarks_by_url()
