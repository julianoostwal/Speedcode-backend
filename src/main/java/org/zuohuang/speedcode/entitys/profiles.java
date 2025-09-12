package org.zuohuang.speedcode.entitys;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import lombok.Data;

import java.sql.Timestamp;
import java.util.UUID;

@Entity
@Data
public class profiles {

    @Id
    private UUID id;

    @Column(nullable = false, columnDefinition = "TIMESTAMP DEFAULT CURRENT_TIMESTAMP", updatable = false)
    private Timestamp created_at;

    @Column(nullable = false, columnDefinition = "TEXT")
    private String nickName;
}
