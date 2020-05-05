import 'package:flutter/material.dart';
import 'package:openid_client/openid_client_io.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:http/http.dart' as http;

void main() => runApp(MyApp());

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flutter Demo',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: MyHomePage(title: 'Flutter Demo Home Page'),
    );
  }
}

class MyHomePage extends StatefulWidget {
  MyHomePage({Key key, this.title}) : super(key: key);
  final String title;

  @override
  _MyHomePageState createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  int _counter = 0;

  void _incrementCounter() {
    setState(() {
      _counter++;
    });
  }

  TokenResponse tokenResponse;

  void _auth() async {
    // create the client
    var uri = new Uri(scheme: "http", host: "192.168.1.107", port: 5000);
    var issuer = await Issuer.discover(uri);
    var client = new Client(issuer, "flutter");

    // create a function to open a browser with an url
    urlLauncher(String url) async {
      if (await canLaunch(url)) {
        await launch(url, forceWebView: true);
      } else {
        throw 'Could not launch $url';
      }
    }

    // create an authenticator
    var authenticator = new Authenticator(client,
        scopes: ["openid", "ApiOne"], port: 4000, urlLancher: urlLauncher);

    // starts the authentication
    var c = await authenticator.authorize();
    tokenResponse = await c.getTokenResponse();

    // close the webview when finished
    closeWebView();
  }

  void _callApi() async {
    var url = 'http://192.168.1.107:5002/secret';
    var access_token = tokenResponse.accessToken;

    var response = await http
        .get(url, headers: {"Authorization": "Bearer $access_token"});
    var body = response.body;
    var a = "";
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.title),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            OutlineButton(
              onPressed: _auth,
              child: Text("Login"),
            ),
            OutlineButton(
              onPressed: _callApi,
              child: Text("Call Api"),
            )
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _incrementCounter,
        tooltip: 'Increment',
        child: Icon(Icons.add),
      ),
    );
  }
}
