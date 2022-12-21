from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


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

    download_month = report_common.remove_quotes(download_month)

    csv_filename = csv
    field_names = [
        "Id", "Metadata", "DownloadMonth", "TotalDownloadsInMonth"
    ]
    key_condition = Key(f'DownloadMonth').eq(f'{download_month}')

    report_common.generate_report(
        aws_profile=aws,
        table_name=f'Songs', index_name=f'DownloadsByMonth',
        sort_column=None, reverse_sort=False,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=None,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_songs_by_month()
