// MIT License
//
// Copyright (c) 2022 Kamil Ercan Turkarslan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

parser grammar DynamoDBReportAntlrParser;

options {
	tokenVocab = DynamoDBReportAntlrLexer;
}

start_rule: dynamo_query+;

dynamo_query:
	REPORT report_name BEGIN_BLOCK select_statement FROM table_name (index_statement | partition_statement) 
	(WHERE where_statement (AND where_statement)*)? END_BLOCK;

report_name: IDENTIFIER;

select_statement: SELECT output_column (COMMA output_column)*;

table_name: IDENTIFIER | string_with_quotes;

output_column: IDENTIFIER (COLUMN (ASCENDING | DESCENDING))?;

partition_statement:
	WITH partition_key_name EQ partition_key_value (
		AND sortkey_statement
	)?;

partition_key_name: IDENTIFIER;
partition_key_value: string_with_quotes | parameter;

index_statement:
	WITH index_name OPEN_PARA key_statement (
		AND sortkey_statement
	)? CLOSE_PARA;

index_name: IDENTIFIER;

key_statement: key_name EQ key_statement_value;

key_name: IDENTIFIER;
key_statement_value: string_with_quotes | parameter;

sortkey_statement:
	simple_sortkey_statement
	| between_sortkey_statement
	| beginswith_sortkey_statement;

simple_sortkey_statement: sortkey_name sortkey_condition value;
between_sortkey_statement:
	sortkey_name BETWEEN first_value AND second_value;
beginswith_sortkey_statement: sortkey_name BEGINS WITH value;

sortkey_name: IDENTIFIER;
sortkey_value: string_with_quotes | parameter;
sortkey_condition: EQ | LTE | LT | GTE | GT;

where_statement:
	between_filter_statement
	| exists_filter_statement
	| contains_filter_statement
	| begins_with_filter_statement
	| simple_filter_statement;

simple_filter_statement: filter_name filter_condition value;
between_filter_statement:
	filter_name BETWEEN first_value AND second_value;
exists_filter_statement: filter_name (EXISTS | NOT EXISTS);
contains_filter_statement:
	filter_name (CONTAINS | NOT CONTAINS) value;
begins_with_filter_statement: filter_name BEGINS WITH value;

filter_name: IDENTIFIER;
filter_condition: EQ | NEQ | LTE | LT | GTE | GT;

value: string_with_quotes | parameter;
first_value: string_with_quotes | parameter;
second_value: string_with_quotes | parameter;
string_with_quotes: STRING_WITH_QUOTES;
parameter: IDENTIFIER;

