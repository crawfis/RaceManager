
/*
DROP ALL tables and triggers.Create, populates all tables and creates triggers.

02/09/2012 Eugene

- Execute from C:\Program Files\Microsoft Visual Studio 10.0\VC>
- To execute commands just cut and paste command line into command window:
*/

sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_RogerS\source\Database_Scripts\drop_triggers_for_SQL_EX.sql
sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_RogerS\source\Database_Scripts\drop_tables_for_SQL_EX.sql
sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_RogerS\source\Database_Scripts\create_tables_for_SQL_EX.sql
sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_RogerS\source\Database_Scripts\create_triggers_for_SQL_EX.sql
sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_RogerS\source\Database_Scripts\create_records_for_SQL_EX.sql


