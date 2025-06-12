package com.echoshift.musyaffa.dto;

import com.echoshift.musyaffa.models.Player;
import java.util.UUID;

public class PlayerDto {
    private UUID id;
    private String username;
    private Integer experience;

    public PlayerDto(Player player) {
        this.id = player.getId();
        this.username = player.getUsername();
        this.experience = player.getExperience();
    }

    public UUID getId() { return id; }
    public void setId(UUID id) { this.id = id; }
    
    public String getUsername() { return username; }
    public void setUsername(String username) { this.username = username; }
    
    public Integer getExperience() { return experience; }
    public void setExperience(Integer experience) { this.experience = experience; }

    @Override
    public String toString() {
        return "PlayerDto{" +
                "id=" + id +
                ", username='" + username + '\'' +
                ", experience=" + experience +
                '}';
    }
}
