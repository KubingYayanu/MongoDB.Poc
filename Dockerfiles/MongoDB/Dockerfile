FROM mongo:6.0.6
COPY mongodb-keyfile /data
COPY init-replica.js /data
RUN chmod 400 /data/mongodb-keyfile
RUN chown 999:999 /data/mongodb-keyfile
RUN chmod 400 /data/init-replica.js
RUN chown 999:999 /data/init-replica.js