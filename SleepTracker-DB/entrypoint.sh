#!/bin/bash

# Start SQL Server
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
echo "Waiting for SQL Server to start..."
until /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -Q "SELECT 1" > /dev/null 2>&1
do
  echo "SQL Server is not yet available. Retrying in 5 seconds..."
  sleep 5
done


echo "SQL Server is now available. Running setup script."

# Run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i /usr/src/app/setup.sql

# In case of a failure, print the SQL Server logs for troubleshooting
if [ $? -ne 0 ]; then
    echo "SQL Server setup failed. Check the SQL Server error logs for details."
    tail -f /var/opt/mssql/log/errorlog
fi

# Keep container running
tail -f /dev/null
