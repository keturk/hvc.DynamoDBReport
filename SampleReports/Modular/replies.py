from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--id", prompt="id", required=True)
@click.option("--reply_date_time", prompt="replyDateTime", required=True)
def generate_replies(aws: str, csv: str, silent: bool,
                     id: str,
                     reply_date_time: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    id = report_common.remove_quotes(id)
    reply_date_time = report_common.remove_quotes(reply_date_time)

    csv_filename = csv
    field_names = [
        "Id", "ReplyDateTime", "Message", "PostedBy"
    ]
    key_condition = Key(f'Id').eq(f'{id}') & Key('ReplyDateTime').eq(f'{reply_date_time}')

    report_common.generate_report(
        aws_profile=aws,
        table_name=f'Reply', index_name=None,
        sort_column=None, reverse_sort=False,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=None,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_replies()
