FROM mongo:6.0.6

RUN mkdir /data/conf
COPY ./conf/* /data/conf

RUN chown 999:999 /data/conf/*

RUN chmod 400 /data/conf/*