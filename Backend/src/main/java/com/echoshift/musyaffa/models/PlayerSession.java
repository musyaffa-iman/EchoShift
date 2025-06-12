package com.echoshift.musyaffa.models;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "player_sessions")
public class PlayerSession extends BaseEntity {
    
    @Column(name = "session_token", nullable = false, unique = true, length = 255)
    private String sessionToken;
    
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "player_id", nullable = false)
    private Player player;
    
    @Column(name = "is_active", nullable = false, columnDefinition = "BOOLEAN DEFAULT true")
    private Boolean isActive = true;
    
    @Column(name = "created_at", nullable = false)
    private LocalDateTime createdAt;

    // Default constructor
    public PlayerSession() {
        this.createdAt = LocalDateTime.now();
    }

    // Constructor with parameters
    public PlayerSession(String sessionToken, Player player) {
        this.sessionToken = sessionToken;
        this.player = player;
        this.isActive = true;
        this.createdAt = LocalDateTime.now();
    }

    // Getters and setters
    public String getSessionToken() {
        return sessionToken;
    }

    public void setSessionToken(String sessionToken) {
        this.sessionToken = sessionToken;
    }

    public Player getPlayer() {
        return player;
    }

    public void setPlayer(Player player) {
        this.player = player;
    }

    public Boolean getIsActive() {
        return isActive;
    }

    public void setIsActive(Boolean isActive) {
        this.isActive = isActive;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    @Override
    public String toString() {
        return "PlayerSession{" +
                "id=" + getId() +
                ", sessionToken='" + sessionToken + '\'' +
                ", playerId=" + (player != null ? player.getId() : null) +
                ", isActive=" + isActive +
                ", createdAt=" + createdAt +
                '}';
    }
}
