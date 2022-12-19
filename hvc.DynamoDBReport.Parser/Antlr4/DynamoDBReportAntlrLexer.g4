lexer grammar DynamoDBReportAntlrLexer;

EQ:          '==';
NEQ:         '!=';
LTE:         '<=';
LT:          '<';
GTE:         '>=';
GT:          '>';

AND:         'And';
ASCENDING:   'Ascending';
BEGINS:      'Begins';
BETWEEN:     'Between';
CONTAINS:    'Contains';
DESCENDING:  'Descending';
EXISTS:      'Exists';
FILTERS:     'Filters';
FROM:        'From';
KEY:         'Key';
NOT:         'Not';
REPORT:      'Report';
SELECT:      'Select';
USING:       'Using';
WHERE:       'Where';
WITH:        'With';

COMMA:       ',';
COLUMN:      ':';
EQUAL:       '=';
SEMICOLUMN:  ';';

BEGIN_BLOCK: '{';
END_BLOCK:   '}';

OPEN_PARA:   '(';
CLOSE_PARA:  ')';

STRING_WITH_QUOTES: '\'' (~[\\\r\n\f'] )* '\'';

SPACE: [ \t\r\n]+ -> skip;

COMMENT: ( '//' ~[\r\n]* '\r'? '\n' | '/*' .*? '*/') -> skip;

IDENTIFIER: ([A-Z] | [a-z]) ([A-Z] | [a-z] | [0-9] | '-' | '_')*;


