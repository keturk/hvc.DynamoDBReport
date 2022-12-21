import boto3
from prettytable import PrettyTable


def remove_quotes(s):
    if s.startswith('"') and s.endswith('"'):
        return s[1:-1]
    return s


def table_query(table, index_name, key_condition, filter_expression, last_key):
    if index_name is None:
        if last_key is None:
            if filter_expression is None:
                return table.query(KeyConditionExpression=key_condition)
            else:
                return table.query(KeyConditionExpression=key_condition, FilterExpression=filter_expression)
        else:
            if filter_expression is None:
                return table.query(KeyConditionExpression=key_condition, ExclusiveStartKey=last_key)
            else:
                return table.query(KeyConditionExpression=key_condition, ExclusiveStartKey=last_key,
                                   FilterExpression=filter_expression)
    else:
        if last_key is None:
            if filter_expression is None:
                return table.query(IndexName=index_name, KeyConditionExpression=key_condition)
            else:
                return table.query(IndexName=index_name, KeyConditionExpression=key_condition,
                                   FilterExpression=filter_expression)
        else:
            if filter_expression is None:
                return table.query(IndexName=index_name, KeyConditionExpression=key_condition,
                                   ExclusiveStartKey=last_key)
            else:
                return table.query(IndexName=index_name, KeyConditionExpression=key_condition,
                                   ExclusiveStartKey=last_key,
                                   FilterExpression=filter_expression)


def generate_report(aws_profile,
                    table_name, index_name,
                    sort_column, reverse_sort,
                    field_names,
                    key_condition, filter_expression,
                    csv_filename=None, silent=False):
    session = boto3.Session(profile_name=aws_profile)
    dynamodb = session.resource('dynamodb')

    table = dynamodb.Table(table_name)

    record_count = 0
    has_more_data = True
    last_key = None

    output_table = PrettyTable()
    output_table.field_names = field_names
    output_table.sortby = sort_column
    output_table.reversesort = reverse_sort
    if not silent:
        print(f"Querying {table.table_name} table")

    while has_more_data:
        response = table_query(table, index_name, key_condition, filter_expression, last_key)

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

