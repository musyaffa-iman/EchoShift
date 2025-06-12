package com.echoshift.musyaffa;

import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;

import javax.sql.DataSource;
import java.sql.Connection;

@SpringBootApplication
public class MusyaffaApplication {

	public static void main(String[] args) {
		SpringApplication.run(MusyaffaApplication.class, args);
	}
	
	@Bean
	CommandLineRunner testDatabaseConnection(DataSource dataSource) {
		return args -> {
			System.out.println("Testing database connection...");
			try (Connection connection = dataSource.getConnection()) {
				System.out.println("Database connection successful!");
				System.out.println("Database: " + connection.getCatalog());
				System.out.println("Schema: " + connection.getSchema());
			} catch (Exception e) {
				System.err.println("Database connection failed: " + e.getMessage());
				e.printStackTrace();
			}
		};
	}

}
