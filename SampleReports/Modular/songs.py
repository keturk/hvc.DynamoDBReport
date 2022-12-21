from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--id", prompt="id", required=True)
@click.option("--meta_data", prompt="metaData", required=True)
@click.option("--download_timestamp", prompt="downloadTimestamp", required=True)
def generate_songs(aws: str, csv: str, silent: bool,
                   id: str,
                   meta_data: str,
                   download_timestamp: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    id = report_common.remove_quotes(id)
    meta_data = report_common.remove_quotes(meta_data)
    download_timestamp = report_common.remove_quotes(download_timestamp)

    csv_filename = csv
    field_names = [
        "Id", "Metadata", "DownloadTimestamp"
    ]
    key_condition = Key(f'Id').eq(f'{id}') & Key('Metadata').begins_with(f'{meta_data}')
    filter_expression = \
        Attr('DownloadTimestamp').gt(f'{download_timestamp}')
    report_common.generate_report(
        aws_profile=aws,
        table_name=f'Songs', index_name=None,
        sort_column=f'DownloadTimestamp', reverse_sort=False,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=filter_expression,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_songs()
