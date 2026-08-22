.PHONY: dev up down build rebuild restart logs ps migrate shell-db reset-db

dev:
	dotnet run --project BookingSystem.API --launch-profile http

up:
	docker compose up -d

down:
	docker compose down

build:
	docker compose build

rebuild:
	docker compose down && docker compose build --no-cache && docker compose up -d

restart:
	docker compose down
	docker compose build
	docker compose up -d

logs:
	docker compose logs -f

ps:
	docker compose ps

migrate:
	docker compose exec api dotnet ef database update

shell-db:
	docker compose exec db psql -U bookinguser -d bookingdb

# Drops the database so DbUp re-runs Migrations/ — needed after editing a seed.
reset-db:
	docker compose down -v
	docker compose up -d db
