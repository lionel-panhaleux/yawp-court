.PHONY: deploy

REMOTE ?= krcg.org:projects/yawp.krcg.org/dist

deploy:
	rsync -rptov --delete-after -e ssh ./src/ ${REMOTE}
