import click
import boto3
from boto3.dynamodb.conditions import Attr, Key
from prettytable import PrettyTable


@click.command()
@click.option("--aws", help="AWS Profile", required=False, default="default")
@click.option("--csv", help="CSV output filename", required=False)
@click.option("--silent", is_flag=True, help="Don't output to console", required=False)
@click.option("--pk", prompt="pk", required=True)
@click.option("--sk", prompt="sk", required=True)
def generate_credit_card_offers(aws: str, csv: str, silent: bool,
                                pk: str,
                                sk: str):
    if silent and csv is None:
        print("You must specify either a CSV output file or the silent option")
        return

    if pk.startswith('"') and pk.endswith('"'):
        pk = pk[1:-1]
    if sk.startswith('"') and sk.endswith('"'):
        sk = sk[1:-1]

    csv_filename = csv

    output_table = PrettyTable()
    output_table.field_names = [
        "PK", "SK", "CreatedBy", "CreatedDate", "LastModified", "OfferDescription", "OfferEffectiveDate", "OfferExpiryDate", "OfferId", "OfferSubType", "OfferType", "OfferUrl"
    ]
    output_table.sortby = 'LastModified'
    output_table.reversesort = True

    session = boto3.Session(profile_name=aws)
    dynamodb = session.resource('dynamodb')

    table = dynamodb.Table(f'CreditCardOffers')
    if not silent:
        print(f"Querying {table.table_name} table")

    record_count = 0
    has_more_data = True
    last_key = None
    key_condition = Key(f'PK').eq(f'{pk}') & Key('SK').begins_with(f'{sk}')

    while has_more_data:
        if last_key:
            response = table.query(
                KeyConditionExpression=key_condition,
                ExclusiveStartKey=last_key
            )
        else:
            response = table.query(
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
    generate_credit_card_offers()
