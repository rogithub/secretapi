# secretapi
api designed to store secret string content

## Install
In order to install SQLite library uncomment
nugget.config. To install the rest of the liraries
add standard nugget.org repo.


## Token
When trying token from https://localhost:7070/swagger
url paste token as


bearer {token}

## Curl
If you are testing localhost with https you can
use -k in curl so that ignores self signed ssl certs

## podman
podman build -f Containerfile -t secret-api-img
podman images
podman run -d --name secret-api -p 5000:5000 secret-api-img
podman ps
podman logs secret-api
podman exec -it secret-api bash





