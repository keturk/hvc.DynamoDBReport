from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--alias", prompt="alias", required=True)
def generate_employees(aws: str, csv: str, silent: bool,
                       alias: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    alias = report_common.remove_quotes(alias)

    csv_filename = csv
    field_names = [
        "LoginAlias", "FirstName", "LastName", "ManagerLoginAlias", "Skills"
    ]
    key_condition = Key(f'LoginAlias').eq(f'{alias}')

    report_common.generate_report(
        aws_profile=aws,
        table_name=f'Employee', index_name=None,
        sort_column=None, reverse_sort=False,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=None,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_employees()
