from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--posted_by", prompt="postedBy", required=True)
@click.option("--message", prompt="message", required=True)
def generate_replies_posted_by(aws: str, csv: str, silent: bool,
                               posted_by: str,
                               message: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    posted_by = report_common.remove_quotes(posted_by)
    message = report_common.remove_quotes(message)

    csv_filename = csv
    field_names = [
        "Id", "ReplyDateTime", "Message", "PostedBy"
    ]
    key_condition = Key(f'PostedBy').eq(f'{posted_by}') & Key('Message').begins_with(f'{message}')

    report_common.generate_report(
        aws_profile=aws,
        table_name=f'Reply', index_name=f'PostedBy-Message-Index',
        sort_column=None, reverse_sort=False,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=None,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_replies_posted_by()
