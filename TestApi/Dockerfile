FROM microsoft/dotnet:1.0.1-sdk-projectjson

RUN apt-get update -y
RUN apt-get upgrade -y

RUN apt-get install -y dos2unix

ENV APP_NAME TestApi
ENV APP_PATH /usr/test_api

ENV SOURCE_RELATIVE_PATH .

RUN mkdir $APP_PATH

# Set the timezone.
RUN echo 'Europe/London' > /etc/timezone && dpkg-reconfigure -f noninteractive tzdata

RUN mkdir $APP_PATH/src/

COPY $SOURCE_RELATIVE_PATH/. $APP_PATH/src

RUN dotnet restore $APP_PATH/src

RUN dotnet publish $APP_PATH/src --output $APP_PATH/

COPY run.sh $APP_PATH/

WORKDIR $APP_PATH/

ENV LC_ALL en_US.UTF-8

ENV LANG en_US.UTF-8
ENV LANGUAGE en_US.UTF-8

RUN dos2unix -ascii run.sh

RUN chmod +x run.sh

ENV MAIN_SCRIPT_PATH run.sh

EXPOSE 5000

ENTRYPOINT ["./run.sh"]
CMD ["dotnet", "src.dll"]