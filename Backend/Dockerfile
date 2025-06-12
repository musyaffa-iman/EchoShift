# Use the OpenJDK image you already pulled (but let's use a more stable version)
FROM openjdk:25-jdk

# Set working directory
WORKDIR /app

# Copy Maven wrapper and pom.xml for better layer caching
COPY .mvn/ .mvn/
COPY mvnw pom.xml ./

# Make mvnw executable (important for Windows)
RUN chmod +x ./mvnw

# Download dependencies (this layer will be cached if pom.xml doesn't change)
RUN ./mvnw dependency:go-offline -B

# Copy source code
COPY src ./src

# Build the application
RUN ./mvnw clean package -DskipTests

# Expose port 8080
EXPOSE 8080

# Run the JAR file (based on your folder structure, the JAR should be in target/)
CMD ["java", "-jar", "target/echoshift-be.jar"]