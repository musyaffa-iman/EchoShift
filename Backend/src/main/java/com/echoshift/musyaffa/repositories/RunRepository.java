package com.echoshift.musyaffa.repositories;

import com.echoshift.musyaffa.models.Run;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public interface RunRepository extends JpaRepository<Run, UUID> {
    List<Run> findByPlayerIdOrderByScoreDesc(UUID playerId);
}
