set -euxo pipefail

source ./config/.env

# Push auth service to ECR
aws ecr get-login-password --region eu-central-1 | docker login --username AWS --password-stdin $REPOSITORY
docker build -t $IMAGE_NAME:$IMAGE_TAG .
docker tag $IMAGE_NAME:$IMAGE_TAG $REPOSITORY/$IMAGE_NAME:$IMAGE_TAG
docker push $REPOSITORY/$IMAGE_NAME:$IMAGE_TAG

# Cleanup
docker image rm -f $IMAGE_NAME:$IMAGE_TAG $REPOSITORY/$IMAGE_NAME:$IMAGE_TAG
rm -r bin obj