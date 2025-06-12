package com.echoshift.musyaffa.repositories;

import com.echoshift.musyaffa.models.PlayerSession;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface PlayerSessionRepository extends JpaRepository<PlayerSession, UUID> {
    
    /**
     * Find an active session by session token
     */
    Optional<PlayerSession> findBySessionTokenAndIsActive(String sessionToken, Boolean isActive);
    
    /**
     * Find an active session for a specific player
     */
    Optional<PlayerSession> findByPlayerIdAndIsActive(UUID playerId, Boolean isActive);
    
    /**
     * Find all active sessions for a specific player
     */
    List<PlayerSession> findAllByPlayerIdAndIsActive(UUID playerId, Boolean isActive);
    
    /**
     * Check if a session token exists and is active
     */
    boolean existsBySessionTokenAndIsActive(String sessionToken, Boolean isActive);
    
    /**
     * Deactivate all sessions for a specific player (useful for logout from all devices)
     */
    @Modifying
    @Transactional
    @Query("UPDATE PlayerSession ps SET ps.isActive = false WHERE ps.player.id = :playerId AND ps.isActive = true")
    void deactivateAllPlayerSessions(@Param("playerId") UUID playerId);
    
    /**
     * Deactivate a specific session by token
     */
    @Modifying
    @Transactional
    @Query("UPDATE PlayerSession ps SET ps.isActive = false WHERE ps.sessionToken = :sessionToken AND ps.isActive = true")
    void deactivateSessionByToken(@Param("sessionToken") String sessionToken);
    
    /**
     * Clean up old inactive sessions (for maintenance)
     */
    @Modifying
    @Transactional
    @Query("DELETE FROM PlayerSession ps WHERE ps.isActive = false")
    void deleteInactiveSessions();
}
