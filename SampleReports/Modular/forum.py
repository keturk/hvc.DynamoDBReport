from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--pk", prompt="pk", required=True)
def generate_forum(aws: str, csv: str, silent: bool,
                   pk: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    pk = report_common.remove_quotes(pk)

    csv_filename = csv
    field_names = [
        "ForumName", "Category", "Messages", "Threads", "Views"
    ]
    key_condition = Key(f'ForumName').eq(f'{pk}')

    report_common.generate_report(
        aws_profile=aws,
        table_name=f'Forum', index_name=None,
        sort_column=None, reverse_sort=False,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=None,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_forum()
