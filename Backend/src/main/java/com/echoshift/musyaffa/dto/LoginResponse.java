package com.echoshift.musyaffa.dto;

import com.echoshift.musyaffa.models.Player;

public class LoginResponse {
    private Player player;
    private String sessionToken;

    // Default constructor
    public LoginResponse() {
    }

    // Constructor with parameters
    public LoginResponse(Player player, String sessionToken) {
        this.player = player;
        this.sessionToken = sessionToken;
    }

    // Getters and setters
    public Player getPlayer() {
        return player;
    }

    public void setPlayer(Player player) {
        this.player = player;
    }

    public String getSessionToken() {
        return sessionToken;
    }

    public void setSessionToken(String sessionToken) {
        this.sessionToken = sessionToken;
    }

    @Override
    public String toString() {
        return "LoginResponse{" +
                "player=" + (player != null ? player.getUsername() : null) +
                ", sessionToken='" + sessionToken + '\'' +
                '}';
    }
}
