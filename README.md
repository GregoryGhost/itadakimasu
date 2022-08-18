# Itadakimasu
It's a project about automating collective food order from different food delivery places.

## How to run
Open terminal in root project folder. Run docker compose file:
>docker compose --env-file ./Configs/Envs/dev.env build

>docker compose --env-file ./Configs/Envs/dev.env up

## Notes
To run independently products synchronize saga.
Open terminal in root project folder. Run docker compose file:
>docker compose --env-file ./Configs/Envs/dev.env -f ./docker-compose-products-sync-saga.yml build

>docker compose --env-file ./Configs/Envs/dev.env -f ./docker-compose-products-sync-saga.yml up