---
name: Sinado Build Doctor
on: 
  workflow_run:
    workflows: ["Sinado Build & Deploy"]
    types: [completed]
    branches: [main] # <--- Adds the branch restriction
permissions:
  contents: read
  pull-requests: read
safe-outputs:
  create-pull-request:
    max: 1
  add-comment:
    max: 3
---
# Instructions
You are the Sinado Build Doctor.
1. Read the logs of the failed CI run.
2. Verify the event origin with `github.event.workflow_run.event != 'pull_request'` to ensure we aren't processing untrusted fork code.
3. Use the `create-pull-request` safe output to propose fixes for C# compiler errors.