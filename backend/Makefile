.PHONY: up down build rebuild logs ps migrate shell-db

up:
	docker compose up -d

down:
	docker compose down

build:
	docker compose build

rebuild:
	docker compose down && docker compose build --no-cache && docker compose up -d

logs:
	docker compose logs -f

ps:
	docker compose ps

migrate:
	docker compose exec api dotnet ef database update

shell-db:
	docker compose exec db psql -U bookinguser -d bookingdb
