
#!/bin/bash

set -e

cd ./BTCPayServer.Plugins.Kratos

PUBL=bin/publish
DIST=bin/packed
DEST=~/.btcpayserver/Plugins

rm -rf $DIST/*
mkdir -p $DEST/BTCPayServer.Plugins.Kratos
rm -rf $DEST/BTCPayServer.Plugins.Kratos/*
dotnet publish -c Release -o $PUBL/BTCPayServer.Plugins.Kratos
dotnet run --project ../BTCPayServer.PluginPacker $(pwd)/$PUBL/BTCPayServer.Plugins.Kratos BTCPayServer.Plugins.Kratos $(pwd)/$DIST

cp -R $PUBL/BTCPayServer.Plugins.Kratos/* $DEST/BTCPayServer.Plugins.Kratos

cd ..