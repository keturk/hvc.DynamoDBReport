from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--customer_id", prompt="customerId", required=True)
@click.option("--update_date", prompt="updateDate", required=True)
def generate_customer_bookmarks_by_customer_folder(aws: str, csv: str, silent: bool,
                                                   customer_id: str,
                                                   update_date: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    customer_id = report_common.remove_quotes(customer_id)
    update_date = report_common.remove_quotes(update_date)

    csv_filename = csv
    field_names = [
        "customerId", "sk", "createDate", "description", "folder", "title", "updateDate", "url"
    ]
    key_condition = Key(f'customerId').eq(f'{customer_id}')
    filter_expression = \
        Attr('title').begins_with('AWS') & \
        Attr('updateDate').gt(f'{update_date}')
    report_common.generate_report(
        aws_profile=aws,
        table_name=f'CustomerBookmark', index_name=f'ByCustomerFolder',
        sort_column=f'createDate', reverse_sort=True,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=filter_expression,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_customer_bookmarks_by_customer_folder()
