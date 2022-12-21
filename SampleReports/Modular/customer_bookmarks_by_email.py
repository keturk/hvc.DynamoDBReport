from common import report_common
from boto3.dynamodb.conditions import Attr, Key
import click


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--email", prompt="email", required=True)
def generate_customer_bookmarks_by_email(aws: str, csv: str, silent: bool,
                                         email: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    email = report_common.remove_quotes(email)

    csv_filename = csv
    field_names = [
        "customerId", "sk", "email", "fullName", "userPreferences"
    ]
    key_condition = Key(f'email').eq(f'{email}')

    report_common.generate_report(
        aws_profile=aws,
        table_name=f'CustomerBookmark', index_name=f'ByEmail',
        sort_column=f'email', reverse_sort=True,
        field_names=field_names,
        key_condition=key_condition,
        filter_expression=None,
        csv_filename=csv_filename, silent=silent)


if __name__ == "__main__":
    generate_customer_bookmarks_by_email()
