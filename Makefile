# Environnement utilisé : dev, test ou prod
ENV ?=

COMPOSE = docker compose \
	-p unisearch-$(ENV) \
	--env-file .env.$(ENV) \
	-f docker-compose.yml \
	-f docker-compose.$(ENV).yml

TEST_COMPOSE = docker compose \
	-p unisearch-test \
	--env-file .env.test \
	-f docker-compose.yml \
	-f docker-compose.test.yml


.PHONY: up down build rebuild logs ps clean prune check-env test


check-env:
	@if [ "$(ENV)" != "dev" ] && \
	    [ "$(ENV)" != "prod" ]; then \
		echo "Erreur : ENV doit être dev ou prod."; \
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

test:
	@$(TEST_COMPOSE) up --build \
		--abort-on-container-exit \
		--exit-code-from api \
		--no-attach mongodb > /tmp/unisearch-tests.log 2>&1; \
	status=$$?; \
	cat /tmp/unisearch-tests.log; \
	awk ' \
		/Passed!|Failed!/ { \
			reports++; \
			for (i = 1; i <= NF; i++) { \
				if ($$i == "Failed:") failed += $$(i+1); \
				if ($$i == "Passed:") passed += $$(i+1); \
				if ($$i == "Skipped:") skipped += $$(i+1); \
			} \
		} \
		END { \
			if (reports == 0) { \
				printf "\n\033[1;31m✘ Aucun résultat de test disponible (échec avant le rapport xUnit).\033[0m\n"; \
			} else { \
				printf "\n\033[1m=== RÉSULTATS DES TESTS ===\033[0m\n"; \
				printf "\033[32m✔ Réussis : %d\033[0m\n", passed; \
				printf "\033[31m✘ Échoués : %d\033[0m\n", failed; \
				printf "\033[33m➜ Ignorés : %d\033[0m\n", skipped; \
			} \
		} \
	' /tmp/unisearch-tests.log; \
	exit $$status
