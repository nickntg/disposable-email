const http = require('http');
const https = require('https');
const { URL } = require('url');

exports.register = function () {
  this.loginfo('queue/http_webhook loaded');
};

exports.hook_queue = function (next, connection) {
  const txn = connection.transaction;
  const webhookUrl = process.env.WEBHOOK_URL;

  if (!webhookUrl) {
    connection.logwarn('WEBHOOK_URL not set; accepting message without POSTing');
    return next(OK);
  }

  let url;
  try {
    url = new URL(webhookUrl);
  } catch (e) {
    connection.logerror(`Invalid WEBHOOK_URL (${webhookUrl}): ${e.message}`);
    return next(OK);
  }

  txn.message_stream.get_data((data) => {
    const rawBuf = Buffer.isBuffer(data) ? data : Buffer.from(data || '', 'utf8');

    const payload = {
      envelope: {
        mail_from: txn.mail_from ? txn.mail_from.address() : null,
        rcpt_to: txn.rcpt_to.map(r => r.address()),
      },
      headers: txn.header && txn.header.headers_decoded ? txn.header.headers_decoded : {},
      raw_b64: rawBuf.toString('base64'),
    };

    const body = JSON.stringify(payload);
    const isHttps = url.protocol === 'https:';
    const client = isHttps ? https : http;

    const req = client.request({
      protocol: url.protocol,
      hostname: url.hostname,
      port: url.port || (isHttps ? 443 : 80),
      path: url.pathname + (url.search || ''),
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Content-Length': Buffer.byteLength(body),
      },
      timeout: 5000,
    }, (res) => {
      if (process.env.WEBHOOK_DEBUG === 'true') {
        connection.loginfo(`Webhook responded ${res.statusCode}`);
      }
      next(OK);
    });

    req.on('timeout', () => {
      connection.logerror('Webhook request timed out');
      req.destroy();
      next(OK);
    });

    req.on('error', (err) => {
      connection.logerror(`Webhook error: ${err.message}`);
      next(OK);
    });

    req.write(body);
    req.end();
  });
};
