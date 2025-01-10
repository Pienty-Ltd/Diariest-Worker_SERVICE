#!/bin/sh
cd /app

if [ "x${NR_APP_NAME}" = "x" ]; then
	echo  'NEWRELIC DISABLED'
    if [ "x${APP_TYPE}" = "xworker" ]; then
	  dotnet Pienty.Diariest.Worker.dll
     else
        dotnet Pienty.Diariest.API.dll
    fi
else
	export CORECLR_ENABLE_PROFILING=1
	export CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A}
	export CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent
	export CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so
	export NEW_RELIC_LICENSE_KEY=7a98df2db7256a6e97438bdb3882fe1da6d78a96
	sed -i -E "s/My Application/${NR_APP_NAME}/g" /usr/local/newrelic-dotnet-agent/newrelic.config
	echo  'NEWRELIC ENABLED'
    if [ "x${APP_TYPE}" = "xworker" ]; then
	  dotnet Pienty.Diariest.Worker.dll
    else
        dotnet Pienty.Diariest.API.dll
    fi
fi