#!/bin/bash

# Test script to verify application is running

# Wait for application to start (up to 30 seconds)
for i in {1..30}; do
  if curl -f http://localhost:8080/ 2>/dev/null; then
    echo "Application is running successfully"
    exit 0
  fi
  echo "Waiting for application to start..."
  sleep 1
done

echo "Timeout: Application did not start within 30 seconds"
exit 1