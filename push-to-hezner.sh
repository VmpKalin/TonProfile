set -euxo pipefail

source ./config/.env

echo  | docker login ghcr.io -u vmpkalin --password-stdin
docker build -t $IMAGE_NAME:$IMAGE_TAG .
# docker tag $IMAGE_NAME:$IMAGE_TAG $REPOSITORY/$IMAGE_NAME:$IMAGE_TAG
docker tag $IMAGE_NAME:$IMAGE_TAG ghcr.io/vmpkalin/$IMAGE_NAME:$IMAGE_TAG

docker push ghcr.io/vmpkalin/$IMAGE_NAME:$IMAGE_TAG

# docker push $IMAGE_NAME:$IMAGE_TAG

# Cleanup
# docker image rm -f $IMAGE_NAME:$IMAGE_TAG $REPOSITORY/$IMAGE_NAME:$IMAGE_TAG
# rm -r bin obj