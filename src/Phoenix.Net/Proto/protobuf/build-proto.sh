which protoc
if [ $? != 0 ] ; then
  echo "Must have protoc compiler in your path to generate code"
  exit 1
fi

PROTO_ROOT_DIR=`dirname $0`
PROTO_DIR=$PROTO_ROOT_DIR
CS_DIR=$PROTO_ROOT_DIR/../

set -x 
for f in $PROTO_DIR/*.proto ; do
  protoc -I$PROTO_DIR --csharp_out=$CS_DIR $f
done