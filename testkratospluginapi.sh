# registerflowurl=$(curl -s -X GET -H "Accept: application/json" \
#     		"http://localhost:8088/self-service/registration/api" \
#     		| jq -r '.ui.action')
#echo "$registerflowurl" #https://auth.testnet.btcbank.li/self-service/registration?flow=b708b979-4d24-4559-a2c0-3855fe3af572
loginflowURL=$(curl -s -X GET \
    -H "Accept: application/json" \
    http://localhost:4433/self-service/login/api | jq -r '.ui.action')

echo "Logging in with flow: ${loginflowURL}"

kratostoken=$(curl -s -X POST -H  "Accept: application/json" -H "Content-Type: application/json" \
    -d '{"identifier": "alice@kratos.local", "password": "alice.pass", "method": "password"}' \
    "${loginflowURL}" | jq -r '.session_token')

echo -e "\ntoken is ${kratostoken} with whoami response:"

curl -s -X GET -H "Authorization: Bearer ${kratostoken}" \
	"http://localhost:4433/sessions/whoami" | jq

echo -e "Making request to BTCPayServer on endpoint: api/v1/users/me"

curl -v \
     -H "Authorization: Bearer ${kratostoken}" \
     "http://localhost:14142/api/v1/users/me"