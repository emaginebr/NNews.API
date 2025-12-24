#!/bin/bash

# Generate self-signed certificate for development
# This script creates a certificate for local HTTPS testing

set -e

CERT_DIR="./certs"
CERT_NAME="certificate"
CERT_PASSWORD="${CERTIFICATE_PASSWORD:-pikpro6}"

echo "Creating certificate directory..."
mkdir -p "$CERT_DIR"

echo "Generating self-signed certificate..."
openssl req -x509 -newkey rsa:4096 -sha256 -days 365 \
  -nodes -keyout "$CERT_DIR/$CERT_NAME.key" \
  -out "$CERT_DIR/$CERT_NAME.crt" \
  -subj "/CN=localhost" \
  -addext "subjectAltName=DNS:localhost,DNS:nnews-api,IP:127.0.0.1"

echo "Converting to PFX format..."
openssl pkcs12 -export \
  -out "$CERT_DIR/$CERT_NAME.pfx" \
  -inkey "$CERT_DIR/$CERT_NAME.key" \
  -in "$CERT_DIR/$CERT_NAME.crt" \
  -password pass:$CERT_PASSWORD

echo "Setting permissions..."
chmod 644 "$CERT_DIR/$CERT_NAME.pfx"
chmod 644 "$CERT_DIR/$CERT_NAME.crt"
chmod 600 "$CERT_DIR/$CERT_NAME.key"

echo ""
echo "? Certificate generated successfully!"
echo "Location: $CERT_DIR/$CERT_NAME.pfx"
echo "Password: $CERT_PASSWORD"
echo ""
echo "??  This is a self-signed certificate for development only."
echo "    Do not use in production!"
echo ""
echo "To trust this certificate on your system:"
echo "  Linux:   sudo cp $CERT_DIR/$CERT_NAME.crt /usr/local/share/ca-certificates/ && sudo update-ca-certificates"
echo "  macOS:   sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain $CERT_DIR/$CERT_NAME.crt"
echo "  Windows: Import $CERT_DIR/$CERT_NAME.crt to 'Trusted Root Certification Authorities'"
echo ""
