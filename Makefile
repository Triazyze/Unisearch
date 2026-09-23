# Environnement utilisé : dev, test ou prod
ENV ?=

COMPOSE = docker compose \
	-p unisearch-$(ENV) \
	--env-file .env.$(ENV) \
	-f docker-compose.yml \
	-f docker-compose.$(ENV).yml


.PHONY: up down build rebuild logs ps clean prune check-env


check-env:
	@if [ "$(ENV)" != "dev" ] && \
	    [ "$(ENV)" != "test" ] && \
	    [ "$(ENV)" != "prod" ]; then \
		echo "Erreur : ENV doit être dev, test ou prod."; \
		echo "Exemple : make up ENV=dev"; \
		exit 1; \
	fi


up: check-env
	$(COMPOSE) up


down: check-env
	$(COMPOSE) down


build: check-env
	$(COMPOSE) build


rebuild: check-env
	$(COMPOSE) up --build


logs: check-env
	$(COMPOSE) logs -f


ps: check-env
	$(COMPOSE) ps


clean: check-env
	$(COMPOSE) down -v


prune:
	docker system prune
