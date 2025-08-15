# Dockerfile
FROM node:20-bookworm-slim

RUN apt-get update && apt-get install -y --no-install-recommends \
    python3 build-essential git ca-certificates && \
    rm -rf /var/lib/apt/lists/*

ENV PYTHON=/usr/bin/python3
ENV npm_config_python=/usr/bin/python3

RUN npm install -g --unsafe-perm Haraka && mkdir -p /haraka && haraka -i /haraka

COPY config/ /haraka/config/
COPY plugins/ /haraka/plugins/

WORKDIR /haraka
EXPOSE 25
CMD ["haraka", "-c", "/haraka"]
