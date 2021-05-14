using Google.Protobuf;
using Newtonsoft.Json;
using Phoenix.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using pbc = Google.Protobuf.Collections;

namespace Phoenix.Net.Internal
{
    public class PhoenixClient
    {

        private const string MediaType = "application/x-google-protobuf";
        private const int TimeoutMillis = 30_000;

        private readonly string _requestUrl;
        private readonly CredentialCache _credentialCache;
        public PhoenixClient(string url) : this(url, "", "")
        {

        }
        public PhoenixClient(string url, string userName, string password)
        {
            _requestUrl = url;
            _credentialCache = new CredentialCache();

            _credentialCache.Add(new Uri(url), "Basic", new NetworkCredential(userName, password));
        }


        /// <summary>
        /// This request is used as a short-hand for create a Statement and fetching the first batch 
        /// of results in a single call without any parameter substitution.
        /// </summary>
        public async Task<ExecuteResponse> PrepareAndExecuteRequestAsync(string connectionId, string sql, uint statementId, long maxRowsTotal, int firstFrameMaxSize)
        {
            PrepareAndExecuteRequest req = new PrepareAndExecuteRequest
            {
                Sql = sql,
                ConnectionId = connectionId,
                StatementId = statementId,
                MaxRowsTotal = maxRowsTotal,
                FirstFrameMaxSize = firstFrameMaxSize
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "PrepareAndExecuteRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "PrepareAndExecuteRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ExecuteResponse res = ExecuteResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to open a new Connection in the Phoenix query server.
        /// </summary>
        public async Task<OpenConnectionResponse> OpenConnectionRequestAsync(string connectionId, pbc::MapField<string, string> info)
        {
            OpenConnectionRequest req = new OpenConnectionRequest
            {
                ConnectionId = connectionId,
            };
            req.Info.Add(info);


            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "OpenConnectionRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "OpenConnectionRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    OpenConnectionResponse res = OpenConnectionResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to ensure that the client and server have a consistent view of the database properties.
        /// </summary>
        public async Task<ConnectionSyncResponse> ConnectionSyncRequestAsync(string connectionId, ConnectionProperties props)
        {
            ConnectionSyncRequest req = new ConnectionSyncRequest
            {
                ConnectionId = connectionId,
                ConnProps = props
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "ConnectionSyncRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "ConnectionSyncRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ConnectionSyncResponse res = ConnectionSyncResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to create a new Statement in the Phoenix query server.
        /// </summary>
        public async Task<CreateStatementResponse> CreateStatementRequestAsync(string connectionId)
        {
            CreateStatementRequest req = new CreateStatementRequest
            {
                ConnectionId = connectionId
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "CreateStatementRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "CreateStatementRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    CreateStatementResponse res = CreateStatementResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to close the Statement object in the Phoenix query server identified by the given IDs.
        /// </summary>
        public async Task<CloseStatementResponse> CloseStatementRequestAsync(string connectionId, uint statementId)
        {
            CloseStatementRequest req = new CloseStatementRequest
            {
                ConnectionId = connectionId,
                StatementId = statementId
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "CloseStatementRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "CloseStatementRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    CloseStatementResponse res = CloseStatementResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to close the Connection object in the Phoenix query server identified by the given IDs.
        /// </summary>
        public async Task<CloseConnectionResponse> CloseConnectionRequestAsync(string connectionId)
        {
            CloseConnectionRequest req = new CloseConnectionRequest
            {
                ConnectionId = connectionId
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "CloseConnectionRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "CloseConnectionRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    CloseConnectionResponse res = CloseConnectionResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to issue a commit on the Connection in the Phoenix query server identified by the given ID.
        /// </summary>
        public async Task<CommitResponse> CommitRequestAsync(string connectionId)
        {
            CommitRequest req = new CommitRequest
            {
                ConnectionId = connectionId
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "CommitRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "CommitRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    CommitResponse res = CommitResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to issue a rollback on the Connection in the Phoenix query server identified by the given ID.
        /// </summary>
        public async Task<RollbackResponse> RollbackRequestAsync(string connectionId)
        {
            RollbackRequest req = new RollbackRequest
            {
                ConnectionId = connectionId
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "RollbackRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "RollbackRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    RollbackResponse res = RollbackResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to create create a new Statement with the given query in the Phoenix query server.
        /// </summary>
        public async Task<PrepareResponse> PrepareRequestAsync(string connectionId, string sql, long maxRowsTotal)
        {
            PrepareRequest req = new PrepareRequest
            {
                ConnectionId = connectionId,
                Sql = sql,
                MaxRowsTotal = maxRowsTotal
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "PrepareRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "PrepareRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    PrepareResponse res = PrepareResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to execute a PreparedStatement, optionally with values to bind to the parameters in the Statement.
        /// </summary>
        public async Task<ExecuteResponse> ExecuteRequestAsync(StatementHandle statementHandle, pbc::RepeatedField<TypedValue> parameterValues, int firstFrameMaxSize, bool hasParameterValues)
        {
            ExecuteRequest req = new ExecuteRequest
            {
                StatementHandle = statementHandle,
                FirstFrameMaxSize = firstFrameMaxSize,
                HasParameterValues = hasParameterValues
            };
            req.ParameterValues.AddRange(parameterValues);
            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "ExecuteRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "ExecuteRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            statementHandle.ConnectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ExecuteResponse res = ExecuteResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to fetch the tables available in this database filtered by the provided criteria.
        /// </summary>
        public async Task<ResultSetResponse> TablesRequestAsync(string catalog, string schemaPattern, string tableNamePattern, pbc::RepeatedField<string> typeList, bool hasTypeList, string connectionId)
        {
            TablesRequest req = new TablesRequest
            {
                Catalog = catalog,
                SchemaPattern = schemaPattern,
                TableNamePattern = tableNamePattern,
                HasTypeList = hasTypeList,
                ConnectionId = connectionId
            };
            req.TypeList.AddRange(typeList);

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "TablesRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "TablesRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ResultSetResponse res = ResultSetResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to fetch the available catalog names in the database.
        /// </summary>
        public async Task<ResultSetResponse> CatalogsRequestAsync(string connectionId)
        {
            CatalogsRequest req = new CatalogsRequest
            {
                ConnectionId = connectionId
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "CatalogsRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "CatalogsRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ResultSetResponse res = ResultSetResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to fetch the table types available in this database.
        /// </summary>
        public async Task<ResultSetResponse> TableTypesRequestAsync(string connectionId)
        {
            TableTypesRequest req = new TableTypesRequest
            {
                ConnectionId = connectionId
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "TableTypesRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "TableTypesRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ResultSetResponse res = ResultSetResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to fetch the table types available in this database.
        /// </summary>
        public async Task<ResultSetResponse> SchemasRequestAsync(string catalog, string schemaPattern, string connectionId)
        {
            SchemasRequest req = new SchemasRequest
            {
                Catalog = catalog,
                SchemaPattern = schemaPattern,
                ConnectionId = connectionId
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "SchemasRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "SchemasRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ResultSetResponse res = ResultSetResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to fetch the columns in this database.
        /// </summary>
        public async Task<ResultSetResponse> ColumnsRequestAsync(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern, string connectionId)
        {
            ColumnsRequest req = new ColumnsRequest
            {
                Catalog = catalog,
                SchemaPattern = schemaPattern,
                TableNamePattern = tableNamePattern,
                ColumnNamePattern = columnNamePattern,
                ConnectionId = connectionId
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "ColumnsRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "ColumnsRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ResultSetResponse res = ResultSetResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }

        /// <summary>
        /// This request is used to fetch a batch of rows from a Statement previously created.
        /// </summary>
        public async Task<FetchResponse> FetchRequestAsync(string connectionId, uint statementId, ulong offset, int frameMaxSize)
        {
            FetchRequest req = new FetchRequest
            {
                ConnectionId = connectionId,
                StatementId = statementId,
                Offset = offset,
                FrameMaxSize = frameMaxSize
            };

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "FetchRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "FetchRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    FetchResponse res = FetchResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }


        /// <summary>
        /// This request is used as short-hand to create a Statement and execute a batch of updates against that Statement.
        /// </summary>
        public async Task<ExecuteBatchResponse> PrepareAndExecuteBatchRequestAsync(string connectionId, uint statementId, pbc::RepeatedField<string> sqlCommands)
        {
            PrepareAndExecuteBatchRequest req = new PrepareAndExecuteBatchRequest
            {
                ConnectionId = connectionId,
                StatementId = statementId
            };
            req.SqlCommands.AddRange(sqlCommands);

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "PrepareAndExecuteBatchRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "PrepareAndExecuteBatchRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ExecuteBatchResponse res = ExecuteBatchResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }


        /// <summary>
        /// This request is used to execute a batch of updates against a PreparedStatement.
        /// </summary>
        public async Task<ExecuteBatchResponse> ExecuteBatchRequestAsync(string connectionId, uint statementId, pbc::RepeatedField<UpdateBatch> updates)
        {
            ExecuteBatchRequest req = new ExecuteBatchRequest
            {
                ConnectionId = connectionId,
                StatementId = statementId
            };
            req.Updates.AddRange(updates);

            WireMessage msg = new WireMessage
            {
                Name = Constants.WireMessagePrefix + "ExecuteBatchRequest",
                WrappedMessage = req.ToByteString()
            };

            using (Response webResponse = await PostRequestAsync(msg.ToByteArray()))
            {
                if (webResponse.WebResponse.StatusCode != HttpStatusCode.OK)
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ErrorResponse res = ErrorResponse.Parser.ParseFrom(output.WrappedMessage);
                    throw new WebException(
                        string.Format(
                            "ExecuteBatchRequestAsync failed! connectionId: {0}, Response code was: {1}, Response body was: {2}",
                            connectionId,
                            webResponse.WebResponse.StatusCode,
                            res.ToString()));
                }
                else
                {
                    WireMessage output = WireMessage.Parser.ParseFrom(webResponse.WebResponse.GetResponseStream());
                    ExecuteBatchResponse res = ExecuteBatchResponse.Parser.ParseFrom(output.WrappedMessage);
                    return res;
                }
            }
        }


        private async Task<Response> PostRequestAsync(byte[] request)
        {
            Stopwatch watch = Stopwatch.StartNew();
            using var input = new MemoryStream(request);
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(_requestUrl);
            //httpWebRequest.ServicePoint.ReceiveBufferSize = options.ReceiveBufferSize;
            httpWebRequest.ServicePoint.UseNagleAlgorithm = false;
            httpWebRequest.Timeout = TimeoutMillis;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Credentials = _credentialCache;
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = MediaType;
            httpWebRequest.ContentType = MediaType;
            // ignore certificates
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };

            long remainingTime = TimeoutMillis;

            if (input != null)
            {
                // expecting the caller to seek to the beginning or to the location where it needs to be copied from
                Stream req = null;
                try
                {
                    req = await httpWebRequest.GetRequestStreamAsync().WithTimeout(
                                                TimeSpan.FromMilliseconds(remainingTime),
                                                "Waiting for RequestStream");

                    remainingTime = TimeoutMillis - watch.ElapsedMilliseconds;
                    if (remainingTime <= 0)
                    {
                        remainingTime = 0;
                    }

                    await input.CopyToAsync(req).WithTimeout(
                                TimeSpan.FromMilliseconds(remainingTime),
                                "Waiting for CopyToAsync",
                                CancellationToken.None);
                }
                catch (TimeoutException)
                {
                    httpWebRequest.Abort();
                    throw;
                }
                finally
                {
                    if (req != null)
                    {
                        req.Close();
                    }
                }
            }

            try
            {
                remainingTime = TimeoutMillis - watch.ElapsedMilliseconds;
                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                }

                HttpWebResponse response = (HttpWebResponse)await httpWebRequest.GetResponseAsync().WithTimeout(
                                                                TimeSpan.FromMilliseconds(remainingTime),
                                                                "Waiting for GetResponseAsync");
                return new Response()
                {
                    WebResponse = response,
                    RequestLatency = watch.Elapsed
                };
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    WebException we = (WebException)ex;
                    var resp = we.Response as HttpWebResponse;
                    return new Response()
                    {
                        WebResponse = resp,
                        RequestLatency = watch.Elapsed
                    };
                }
                else
                {
                    httpWebRequest.Abort();
                    throw;
                }
            }
        }


    }
}
