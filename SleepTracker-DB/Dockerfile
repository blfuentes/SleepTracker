# Use the official image as a parent image
FROM mcr.microsoft.com/mssql/server:2019-latest

# Set the working directory in the container
WORKDIR /usr/src/app

# Copy the scripts into the container
COPY --chown=mssql:mssql ./setup.sql /usr/src/app/setup.sql
COPY --chown=mssql:mssql ./entrypoint.sh /usr/src/app/entrypoint.sh

# Change ownership of the entrypoint.sh file to root
USER root
RUN tr -d '\r' < /usr/src/app/entrypoint.sh > /tmp/entrypoint.sh && \
    mv /tmp/entrypoint.sh /usr/src/app/entrypoint.sh && \
    chmod +x /usr/src/app/entrypoint.sh


# Set environment variables
ENV SA_PASSWORD=testpassword123!
ENV ACCEPT_EULA=Y

# Expose the default SQL Server port
EXPOSE 1433

# Change back to the mssql user
USER mssql

# Run the script on container startup
CMD /usr/src/app/entrypoint.sh
