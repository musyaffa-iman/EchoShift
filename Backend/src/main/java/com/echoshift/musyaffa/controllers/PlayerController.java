package com.echoshift.musyaffa.controllers;

import com.echoshift.musyaffa.models.Player;
import com.echoshift.musyaffa.models.PlayerSession;
import com.echoshift.musyaffa.repositories.PlayerRepository;
import com.echoshift.musyaffa.repositories.PlayerSessionRepository;
import com.echoshift.musyaffa.dto.LoginRequest;
import com.echoshift.musyaffa.dto.LoginResponse;
import com.echoshift.musyaffa.dto.PlayerDto;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Optional;
import java.util.UUID;

@RestController
@RequestMapping("/api/players")
public class PlayerController {

    @Autowired
    private PlayerRepository playerRepository;
    
    @Autowired
    private PlayerSessionRepository playerSessionRepository;

    @PostMapping
    public ResponseEntity<BaseResponse<LoginResponse>> createPlayer(@RequestBody Player player) {
        System.out.println("Received create player request: " + player);
        
        // Check if username exists
        if (player.getUsername() == null) {
            System.out.println("Username is null");
            BaseResponse<LoginResponse> response = new BaseResponse<>(false, "Username cannot be null", null);
            return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(response);
        }
        
        // Check if username already exists
        if (playerRepository.existsByUsername(player.getUsername())) {
            System.out.println("Username already exists: " + player.getUsername());
            BaseResponse<LoginResponse> response = new BaseResponse<>(false, "Username already exists", null);
            return ResponseEntity.status(HttpStatus.CONFLICT).body(response);
        }
        
        try {
            System.out.println("Saving player: " + player.getUsername());
            Player savedPlayer = playerRepository.save(player);
            System.out.println("Player saved successfully with ID: " + savedPlayer.getId());
            
            // Create session token for auto-login
            String sessionToken = UUID.randomUUID().toString();
            
            // Create new session
            PlayerSession session = new PlayerSession(sessionToken, savedPlayer);
            playerSessionRepository.save(session);
            
            System.out.println("Registration and auto-login successful for user: " + savedPlayer.getUsername() + " with session: " + sessionToken);
            
            // Return LoginResponse instead of just Player
            LoginResponse loginResponse = new LoginResponse(savedPlayer, sessionToken);
            BaseResponse<LoginResponse> response = new BaseResponse<>(true, "Player created and logged in successfully", loginResponse);
            return ResponseEntity.status(HttpStatus.CREATED).body(response);
            
        } catch (Exception e) {
            System.err.println("Error saving player: " + e.getMessage());
            e.printStackTrace();
            BaseResponse<LoginResponse> response = new BaseResponse<>(false, "Error creating player: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }    @PostMapping("/login")
    public ResponseEntity<BaseResponse<LoginResponse>> loginPlayer(@RequestBody LoginRequest loginRequest) {
        System.out.println("Received login request for username: " + loginRequest.getUsername());
        
        // Check if username and password are provided
        if (loginRequest.getUsername() == null || loginRequest.getUsername().trim().isEmpty()) {
            System.out.println("Username is null or empty");
            BaseResponse<LoginResponse> response = new BaseResponse<>(false, "Username cannot be null or empty", null);
            return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(response);
        }
        
        if (loginRequest.getPassword() == null || loginRequest.getPassword().trim().isEmpty()) {
            System.out.println("Password is null or empty");
            BaseResponse<LoginResponse> response = new BaseResponse<>(false, "Password cannot be null or empty", null);
            return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(response);
        }
        
        try {
            // Find player by username
            Optional<Player> playerOptional = playerRepository.findByUsername(loginRequest.getUsername());
            
            if (playerOptional.isPresent()) {
                Player player = playerOptional.get();
                
                // Verify password (in production, use proper password hashing like BCrypt)
                if (player.getPassword().equals(loginRequest.getPassword())) {
                    // Deactivate any existing sessions for this player
                    playerSessionRepository.deactivateAllPlayerSessions(player.getId());
                    
                    // Generate new session token
                    String sessionToken = UUID.randomUUID().toString();
                    
                    // Create new session
                    PlayerSession session = new PlayerSession(sessionToken, player);
                    playerSessionRepository.save(session);
                    
                    System.out.println("Login successful for user: " + player.getUsername() + " with session: " + sessionToken);
                    
                    LoginResponse loginResponse = new LoginResponse(player, sessionToken);
                    BaseResponse<LoginResponse> response = new BaseResponse<>(true, "Login successful", loginResponse);
                    return ResponseEntity.ok(response);
                } else {
                    System.out.println("Invalid password for user: " + loginRequest.getUsername());
                    BaseResponse<LoginResponse> response = new BaseResponse<>(false, "Invalid username or password", null);
                    return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(response);
                }
            } else {
                System.out.println("User not found: " + loginRequest.getUsername());
                BaseResponse<LoginResponse> response = new BaseResponse<>(false, "Invalid username or password", null);
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(response);
            }
        } catch (Exception e) {
            System.err.println("Error during login: " + e.getMessage());
            e.printStackTrace();            BaseResponse<LoginResponse> response = new BaseResponse<>(false, "Error during login: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    @PostMapping("/logout")
    public ResponseEntity<BaseResponse<Void>> logoutPlayer(@RequestHeader("Authorization") String sessionToken) {
        System.out.println("Received logout request for session: " + sessionToken);
        
        if (sessionToken == null || sessionToken.trim().isEmpty()) {
            BaseResponse<Void> response = new BaseResponse<>(false, "Session token is required", null);
            return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(response);
        }
        
        try {
            Optional<PlayerSession> sessionOptional = playerSessionRepository.findBySessionTokenAndIsActive(sessionToken, true);
            
            if (sessionOptional.isPresent()) {
                PlayerSession session = sessionOptional.get();
                session.setIsActive(false);
                playerSessionRepository.save(session);
                
                System.out.println("Logout successful for session: " + sessionToken);
                BaseResponse<Void> response = new BaseResponse<>(true, "Logout successful", null);
                return ResponseEntity.ok(response);
            } else {
                System.out.println("Invalid session token: " + sessionToken);
                BaseResponse<Void> response = new BaseResponse<>(false, "Invalid session token", null);
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(response);
            }
        } catch (Exception e) {
            System.err.println("Error during logout: " + e.getMessage());
            e.printStackTrace();
            BaseResponse<Void> response = new BaseResponse<>(false, "Error during logout: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    @GetMapping("/session/validate")
    public ResponseEntity<BaseResponse<PlayerDto>> validateSession(@RequestHeader("Authorization") String sessionToken) {
        System.out.println("Received session validation request for: " + sessionToken);
        
        if (sessionToken == null || sessionToken.trim().isEmpty()) {
            BaseResponse<PlayerDto> response = new BaseResponse<>(false, "Session token is required", null);
            return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(response);
        }
        
        try {
            Optional<PlayerSession> sessionOptional = playerSessionRepository.findBySessionTokenAndIsActive(sessionToken, true);
            
            if (sessionOptional.isPresent()) {
                Player player = sessionOptional.get().getPlayer();
                System.out.println("Session validation successful for user: " + player.getUsername());
                
                // Convert to DTO to avoid proxy issues
                PlayerDto playerDto = new PlayerDto(player);
                
                BaseResponse<PlayerDto> response = new BaseResponse<>(true, "Session is valid", playerDto);
                return ResponseEntity.ok(response);
            } else {
                System.out.println("Invalid or expired session token: " + sessionToken);
                BaseResponse<PlayerDto> response = new BaseResponse<>(false, "Invalid or expired session", null);
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(response);
            }
        } catch (Exception e) {
            System.err.println("Error during session validation: " + e.getMessage());
            e.printStackTrace();
            BaseResponse<PlayerDto> response = new BaseResponse<>(false, "Error during session validation: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }
    
    @DeleteMapping("/{id}")
    public ResponseEntity<BaseResponse<String>> deletePlayer(@PathVariable UUID id) {
        try {
            if (playerRepository.existsById(id)) {
                playerRepository.deleteById(id);
                BaseResponse<String> response = new BaseResponse<>(true, "Player deleted successfully", "deleted");
                return ResponseEntity.ok(response);
            } else {
                BaseResponse<String> response = new BaseResponse<>(false, "Player not found", null);
                return ResponseEntity.status(HttpStatus.NOT_FOUND).body(response);
            }
        } catch (Exception e) {
            BaseResponse<String> response = new BaseResponse<>(false, "Error deleting player: " + e.getMessage(), null);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }
}
