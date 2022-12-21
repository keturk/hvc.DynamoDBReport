import click
import boto3
from boto3.dynamodb.conditions import Attr, Key
from prettytable import PrettyTable


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--download_month", prompt="downloadMonth", required=True)
def generate_songs_by_month(aws: str, csv: str, silent: bool,
                            download_month: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    if download_month.startswith('"') and download_month.endswith('"'):
        download_month = download_month[1:-1]

    csv_filename = csv

    output_table = PrettyTable()
    output_table.field_names = [
        "Id", "Metadata", "DownloadMonth", "TotalDownloadsInMonth"
    ]
    session = boto3.Session(profile_name=aws)
    dynamodb = session.resource('dynamodb')

    table = dynamodb.Table(f'Songs')
    if not silent:
        print(f"Querying {table.table_name} table")

    record_count = 0
    has_more_data = True
    last_key = None
    key_condition = Key(f'DownloadMonth').eq(f'{download_month}')

    while has_more_data:
        if last_key:
            response = table.query(
                IndexName=f'DownloadsByMonth',
                KeyConditionExpression=key_condition,
                ExclusiveStartKey=last_key
            )
        else:
            response = table.query(
                IndexName=f'DownloadsByMonth',
                KeyConditionExpression=key_condition)

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
    generate_songs_by_month()
