from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--lift", prompt="lift", required=True)
@click.option("--total", prompt="total", required=True)
def generate_ski_lift_by_riders(aws: str, csv: str, silent: bool,
                                lift: str,
                                total: int):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    lift = report_common.remove_quotes(lift)

    csv_filename = csv
    field_names = [
        "Lift", "Metadata", "TotalUniqueLiftRiders"
    ]
    key_condition = Key(f'Lift').eq(f'{lift}') & Key('TotalUniqueLiftRiders').gte(int(total))

    report_common.generate_report(
        aws_profile=aws,
        table_name=f'SkiLifts', index_name=f'SkiLiftsByRiders',
        sort_column=None, reverse_sort=False,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=None,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_ski_lift_by_riders()
