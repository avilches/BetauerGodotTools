FROM barichello/godot-ci:mono-3.3.4
ENV GODOT_VERSION=3.3.4
ENV EXPORT_NAME=Veronenger-3.3.4
RUN mkdir /workspace
WORKDIR /workspace
COPY . .
RUN pwd
RUN ls -la .
RUN ls /root/.local/share/godot/templates/
#RUN mkdir -v -p ~/.local/share/godot/templates
#RUN mv /root/.local/share/godot/templates/${GODOT_VERSION}.stable ~/.local/share/godot/templates/${GODOT_VERSION}.stable
#RUN godot Tests/Runner/RunTests.tscn --no-window
RUN mkdir -v -p build/windows
RUN godot -v --export "Windows Desktop" build/windows/$EXPORT_NAME.exe

