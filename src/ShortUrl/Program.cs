﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ShortUrl;
using ShortUrl.Abstractions;
using ShortUrl.Services;
using ShortUrl.Services.Repository;

var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices(
                        (context, services) => services
                            .AddTransient<DataStore>(_ => new DataStore(false))
                            .AddTransient<IMapUrl, MapUrl>()
                            .AddTransient<IMappedUrl, MappedUrls>()
                            .AddSingleton<Func<Uri, Uri>>(ShortUrlAlgorithm.CreateShortUrl)
                            .AddHostedService<ConsoleUiHostedService>()
                        )
                    .Build();

await host.RunAsync();
return 0;
