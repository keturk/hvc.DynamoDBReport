import click
import boto3
from boto3.dynamodb.conditions import Attr, Key
from prettytable import PrettyTable


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

    if id.startswith('"') and id.endswith('"'):
        id = id[1:-1]

    csv_filename = csv

    output_table = PrettyTable()
    output_table.field_names = [
        "customerId", "sk", "createDate", "description", "folder", "title", "updateDate", "url"
    ]
    output_table.sortby = 'createDate'
    output_table.reversesort = True

    session = boto3.Session(profile_name=aws)
    dynamodb = session.resource('dynamodb')

    table = dynamodb.Table(f'CustomerBookmark')
    if not silent:
        print(f"Querying {table.table_name} table")

    record_count = 0
    has_more_data = True
    last_key = None
    key_condition = Key(f'customerId').eq(f'{id}')
    filter_expression = \
        Attr('title').begins_with('AWS') & \
        Attr('url').begins_with('https:')
    while has_more_data:
        if last_key:
            response = table.query(
                KeyConditionExpression=key_condition,
                FilterExpression=filter_expression,
                ExclusiveStartKey=last_key
            )
        else:
            response = table.query(
                KeyConditionExpression=key_condition,
                FilterExpression=filter_expression)

        for item in response["Items"]:
            row_values = []
            for column_header in output_table.field_names:
                if column_header in item.keys():
                    row_values.append(item[column_header])
                else:
                    row_values.append("")
            output_table.add_row(row_values)
        record_count = record_count + len(response["Items"])
        last_key = response.get('LastEvaluatedKey')
        if not last_key:
            has_more_data = False

    if csv_filename:
        with open(csv_filename, 'w', newline='') as csv_file:
            csv_file.write(output_table.get_csv_string())

    if not silent:
        print(output_table)
        print(f'Number of records: {record_count}')


if __name__ == "__main__":
    generate_customer_bookmarks()
